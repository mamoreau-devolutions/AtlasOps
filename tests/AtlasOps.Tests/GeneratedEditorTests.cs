namespace AtlasOps.Tests;

using AtlasOps.Core;
using AtlasOps.Core.Generated;

[TestClass]
public sealed class GeneratedEditorTests
{
    [TestMethod]
    public void StringField_ValidAndEmptyEdits_UpdateModelAndDirtyState()
    {
        AtlasOpsProject model = new() { Name = "Original", UpdatedAt = ModelTestData.FixedTimestamp };
        AtlasOpsProjectEditorViewModel editor = new(model);
        GeneratedFieldEditorViewModel field = Field(editor, nameof(AtlasOpsProject.Name));

        field.Value = "Changed";

        Assert.AreEqual("Changed", model.Name);
        Assert.AreEqual("Changed", field.Value);
        Assert.IsTrue(editor.HasChanges);
        Assert.IsFalse(field.HasError);
        Assert.AreNotEqual(ModelTestData.FixedTimestamp, model.UpdatedAt);

        editor.AcceptChanges();
        field.Value = string.Empty;

        Assert.AreEqual(string.Empty, model.Name);
        Assert.IsTrue(editor.HasChanges);
        Assert.AreEqual(string.Empty, field.ValidationMessage);
    }

    [TestMethod]
    public void BooleanField_InvalidThenValidEdit_PreservesThenUpdatesModelAndValidation()
    {
        AtlasOpsProject model = new() { IsPinned = false, UpdatedAt = ModelTestData.FixedTimestamp };
        AtlasOpsProjectEditorViewModel editor = new(model);

        AssertInvalidThenValid(
            editor,
            Field(editor, nameof(AtlasOpsProject.IsPinned)),
            "not-a-boolean",
            "true",
            "Enter a valid bool.",
            () => model.IsPinned,
            false,
            true);
    }

    [TestMethod]
    public void IntegerField_InvalidThenValidEdit_PreservesThenUpdatesModelAndValidation()
    {
        AtlasOpsProject model = new() { Priority = 3, UpdatedAt = ModelTestData.FixedTimestamp };
        AtlasOpsProjectEditorViewModel editor = new(model);

        AssertInvalidThenValid(
            editor,
            Field(editor, nameof(AtlasOpsProject.Priority)),
            "3.5",
            "17",
            "Enter a valid int.",
            () => model.Priority,
            3,
            17);
    }

    [TestMethod]
    public void LongField_InvalidThenValidEdit_PreservesThenUpdatesModelAndValidation()
    {
        AtlasOpsQuery model = new() { LastDurationMs = 9L, UpdatedAt = ModelTestData.FixedTimestamp };
        AtlasOpsQueryEditorViewModel editor = new(model);

        AssertInvalidThenValid(
            editor,
            Field(editor, nameof(AtlasOpsQuery.LastDurationMs)),
            "9ms",
            "42000000000",
            "Enter a valid long.",
            () => model.LastDurationMs,
            9L,
            42_000_000_000L);
    }

    [TestMethod]
    public void DoubleField_InvalidThenValidEdit_PreservesThenUpdatesModelAndValidation()
    {
        AtlasOpsHost model = new() { CpuLoad = 1D, UpdatedAt = ModelTestData.FixedTimestamp };
        AtlasOpsHostEditorViewModel editor = new(model);

        AssertInvalidThenValid(
            editor,
            Field(editor, nameof(AtlasOpsHost.CpuLoad)),
            "busy",
            "123",
            "Enter a valid double.",
            () => model.CpuLoad,
            1D,
            123D);
    }

    [TestMethod]
    public void DateTimeOffsetField_InvalidThenValidEdit_PreservesThenUpdatesModelAndValidation()
    {
        DateTimeOffset expected = ModelTestData.FixedTimestamp.AddDays(10);
        AtlasOpsCredential model = new()
        {
            ExpiresAt = ModelTestData.FixedTimestamp,
            UpdatedAt = ModelTestData.FixedTimestamp,
        };
        AtlasOpsCredentialEditorViewModel editor = new(model);

        AssertInvalidThenValid(
            editor,
            Field(editor, nameof(AtlasOpsCredential.ExpiresAt)),
            "tomorrow-ish",
            expected.ToString("O"),
            "Enter a valid DateTimeOffset.",
            () => model.ExpiresAt,
            ModelTestData.FixedTimestamp,
            expected);
    }

    [TestMethod]
    public void AcceptChanges_AndDirectPropertySetter_RespectDirtyTransitions()
    {
        AtlasOpsProject model = new() { Name = "Original", UpdatedAt = ModelTestData.FixedTimestamp };
        AtlasOpsProjectEditorViewModel editor = new(model);
        List<string?> notifications = [];
        editor.PropertyChanged += (_, args) => notifications.Add(args.PropertyName);

        editor.Name = "Original";
        Assert.IsFalse(editor.HasChanges);
        Assert.HasCount(0, notifications);
        Assert.AreEqual(ModelTestData.FixedTimestamp, model.UpdatedAt);

        editor.Name = "Changed directly";
        Assert.IsTrue(editor.HasChanges);
        Assert.AreEqual("Changed directly", model.Name);
        Assert.AreNotEqual(ModelTestData.FixedTimestamp, model.UpdatedAt);
        CollectionAssert.Contains(notifications, nameof(AtlasOpsProjectEditorViewModel.Name));
        CollectionAssert.Contains(notifications, nameof(IGeneratedEditorViewModel.HasChanges));

        editor.AcceptChanges();
        Assert.IsFalse(editor.HasChanges);
        Assert.AreEqual(nameof(IGeneratedEditorViewModel.HasChanges), notifications[^1]);
    }

    [TestMethod]
    public void RepeatedInvalidInput_DoesNotRepeatUnchangedValidationNotifications()
    {
        AtlasOpsProjectEditorViewModel editor = new(new AtlasOpsProject());
        GeneratedFieldEditorViewModel field = Field(editor, nameof(AtlasOpsProject.Priority));
        List<string?> notifications = [];
        field.PropertyChanged += (_, args) => notifications.Add(args.PropertyName);

        field.Value = "invalid";
        notifications.Clear();
        field.Value = "still-invalid";

        Assert.HasCount(0, notifications);
        Assert.IsTrue(field.HasError);
        Assert.AreEqual("Enter a valid int.", field.ValidationMessage);
        Assert.IsFalse(editor.HasChanges);
    }

    private static GeneratedFieldEditorViewModel Field(
        IGeneratedEditorViewModel editor,
        string name)
    {
        return editor.Fields.Single(field => field.Name == name);
    }

    private static void AssertInvalidThenValid<T>(
        IGeneratedEditorViewModel editor,
        GeneratedFieldEditorViewModel field,
        string invalidInput,
        string validInput,
        string expectedValidation,
        Func<T> readModel,
        T originalValue,
        T expectedValue)
    {
        List<string?> notifications = [];
        field.PropertyChanged += (_, args) => notifications.Add(args.PropertyName);

        field.Value = invalidInput;

        Assert.AreEqual(originalValue, readModel());
        Assert.IsFalse(editor.HasChanges);
        Assert.IsTrue(field.HasError);
        Assert.AreEqual(expectedValidation, field.ValidationMessage);
        CollectionAssert.Contains(notifications, nameof(GeneratedFieldEditorViewModel.ValidationMessage));
        CollectionAssert.Contains(notifications, nameof(GeneratedFieldEditorViewModel.HasError));

        notifications.Clear();
        field.Value = validInput;

        Assert.AreEqual(expectedValue, readModel());
        Assert.IsTrue(editor.HasChanges);
        Assert.IsFalse(field.HasError);
        Assert.AreEqual(string.Empty, field.ValidationMessage);
        CollectionAssert.Contains(notifications, nameof(GeneratedFieldEditorViewModel.Value));
        CollectionAssert.Contains(notifications, nameof(GeneratedFieldEditorViewModel.ValidationMessage));
        CollectionAssert.Contains(notifications, nameof(GeneratedFieldEditorViewModel.HasError));
    }
}