namespace AtlasOps.Tests;

using AtlasOps.App.Infrastructure;
using AtlasOps.App.Services;
using AtlasOps.Core;

[TestClass]
public sealed class WaveOneFoundationTests
{
    private string temporaryRoot = null!;

    [TestInitialize]
    public void Initialize()
    {
        this.temporaryRoot = Path.Combine(
            Path.GetTempPath(),
            "AtlasOps.WaveOne.Tests",
            Guid.NewGuid().ToString("N"));
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(this.temporaryRoot))
        {
            Directory.Delete(this.temporaryRoot, recursive: true);
        }
    }

    [TestMethod]
    public async Task CommandRouter_ExecutesRegisteredHandlerAndRaisesEvent()
    {
        AtlasOpsCommandRouter router = new();
        List<AtlasOpsCommandId> executed = [];
        int callCount = 0;
        router.CommandExecuted += (_, commandId) => executed.Add(commandId);
        router.Register(AtlasOpsCommandId.Refresh, _ =>
        {
            callCount++;
            return Task.CompletedTask;
        });

        Assert.IsTrue(router.CanExecute(AtlasOpsCommandId.Refresh));
        await router.ExecuteAsync(AtlasOpsCommandId.Refresh);

        Assert.AreEqual(1, callCount);
        CollectionAssert.AreEqual(new[] { AtlasOpsCommandId.Refresh }, executed);
    }

    [TestMethod]
    public async Task CommandRouter_MissingHandlerFailsExplicitly()
    {
        AtlasOpsCommandRouter router = new();

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => router.ExecuteAsync(AtlasOpsCommandId.Save));

        StringAssert.Contains(exception.Message, "Save");
    }

    [TestMethod]
    public void NotificationService_PublishesSeverityAndTimestamp()
    {
        AtlasOpsNotificationService service = new();
        AtlasOpsNotification? published = null;
        service.NotificationPublished += (_, notification) => published = notification;

        service.Publish("Saved", AtlasOpsNotificationSeverity.Success);

        Assert.IsNotNull(published);
        Assert.AreEqual("Saved", published.Message);
        Assert.AreEqual(AtlasOpsNotificationSeverity.Success, published.Severity);
        Assert.IsTrue(published.CreatedAt > DateTimeOffset.MinValue);
    }

    [TestMethod]
    public async Task DialogService_ForwardsRequestAndResult()
    {
        AtlasOpsDialogRequest? received = null;
        AtlasOpsDialogService service = new((request, _) =>
        {
            received = request;
            return Task.FromResult(true);
        });
        AtlasOpsDialogRequest request = new("Confirm", "Continue?", AtlasOpsNotificationSeverity.Warning);

        bool result = await service.ShowAsync(request);

        Assert.IsTrue(result);
        Assert.AreSame(request, received);
    }

    [TestMethod]
    public async Task SettingsService_RoundTripsSettingsAndLayoutAtomically()
    {
        AtlasOpsSettingsService service = new(this.temporaryRoot);
        AtlasOpsSettings settings = new()
        {
            WorkspaceName = "Wave 1",
            Theme = "Dark",
            LocalDataPath = this.temporaryRoot,
        };
        AtlasOpsLayoutState layout = new()
        {
            SelectedRegion = "Dashboard",
            SelectedModelType = "AtlasOpsProject",
            NavigationWidth = 220,
            DetailsWidth = 340,
            IsNavigationCollapsed = true,
        };

        await service.SaveSettingsAsync(settings);
        await service.SaveLayoutAsync(layout);

        AtlasOpsSettings loadedSettings = await service.LoadSettingsAsync();
        AtlasOpsLayoutState loadedLayout = await service.LoadLayoutAsync();

        Assert.AreEqual("Wave 1", loadedSettings.WorkspaceName);
        Assert.AreEqual("Dark", loadedSettings.Theme);
        Assert.AreEqual("Dashboard", loadedLayout.SelectedRegion);
        Assert.AreEqual("AtlasOpsProject", loadedLayout.SelectedModelType);
        Assert.AreEqual(340, loadedLayout.DetailsWidth);
        Assert.IsTrue(loadedLayout.IsNavigationCollapsed);
    }

    [TestMethod]
    public async Task SettingsService_MissingFilesReturnsDefaults()
    {
        AtlasOpsSettingsService service = new(this.temporaryRoot);

        AtlasOpsSettings settings = await service.LoadSettingsAsync();
        AtlasOpsLayoutState layout = await service.LoadLayoutAsync();

        Assert.AreEqual("AtlasOps Workspace", settings.WorkspaceName);
        Assert.AreEqual("Workspace", layout.SelectedRegion);
        Assert.AreEqual(236, layout.NavigationWidth);
    }
}