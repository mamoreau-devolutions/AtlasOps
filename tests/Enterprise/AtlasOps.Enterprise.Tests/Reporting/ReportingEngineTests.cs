namespace AtlasOps.Enterprise.Tests.Reporting;

using AtlasOps.Enterprise.Contracts.Reporting;
using AtlasOps.Enterprise.Core.Reporting;
using AtlasOps.Enterprise.Core.Scenarios;

[TestClass]
public sealed class ReportingEngineTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-10-01T12:00:00Z");

    [TestMethod]
    public void Evaluate_ArithmeticAndBooleanExpression_UsesOperatorPrecedence()
    {
        ReportExpressionCompiler compiler = new();
        ReportRow row = new(
            "sample",
            new Dictionary<string, object?>
            {
                ["age"] = 30m,
                ["severity"] = "high",
            });

        ReportExpressionResult result = compiler.Evaluate("age / 2 + 5 == 20 and severity == 'high'", row);

        Assert.IsTrue(result.IsValid);
        bool value = Assert.IsInstanceOfType<bool>(result.Value);
        Assert.IsTrue(value);
        Assert.IsEmpty(result.Diagnostics);
    }

    [TestMethod]
    public void Evaluate_DivisionByZero_ReturnsPositionalDiagnostic()
    {
        ReportExpressionCompiler compiler = new();
        ReportRow row = new("sample", new Dictionary<string, object?> { ["value"] = 10m });

        ReportExpressionResult result = compiler.Evaluate("value / 0", row);

        Assert.IsFalse(result.IsValid);
        ReportDiagnostic diagnostic = Assert.ContainsSingle(result.Diagnostics);
        Assert.AreEqual("expression.divide.zero", diagnostic.Code);
        Assert.IsNotNull(diagnostic.Position);
    }

    [TestMethod]
    public void Execute_GroupedProductionReport_AggregatesCountAndAverage()
    {
        ReportDefinition report = EnterpriseScenarioCatalog.CreateReports()[0];
        IReadOnlyList<ReportRow> rows = EnterpriseScenarioCatalog.CreateOperationalRows(Now);
        ReportEngine engine = new();

        ReportResult result = engine.Execute(report, rows);

        Assert.AreEqual(6, result.InputRowCount);
        Assert.AreEqual(4, result.FilteredRowCount);
        Assert.HasCount(2, result.Rows);
        ReportResultRow incidentGroup = Assert.ContainsSingle(
            result.Rows.Where(row => string.Equals(Convert.ToString(row.Dimensions["kind"]), "incident", StringComparison.Ordinal)));
        Assert.AreEqual(2, incidentGroup.Values["count"]);
    }

    [TestMethod]
    public void Materialize_MatchingReport_ReturnsPrimaryMeasure()
    {
        ReportDefinition report = EnterpriseScenarioCatalog.CreateReports()[0];
        ReportEngine engine = new();
        ReportResult reportResult = engine.Execute(report, EnterpriseScenarioCatalog.CreateOperationalRows(Now));
        DashboardWidgetDefinition widget = new("production-count", "Production count", DashboardWidgetKind.Metric, report.Id, "count");
        DashboardMaterializer materializer = new();

        DashboardWidgetResult result = materializer.Materialize(widget, reportResult);

        Assert.IsNotNull(result.PrimaryValue);
        Assert.IsNotEmpty(result.Rows);
    }

    [TestMethod]
    public void Execute_IncidentReport_ProjectsCalculatedAge()
    {
        ReportDefinition report = EnterpriseScenarioCatalog.CreateReports()[1];
        ReportEngine engine = new();

        ReportResult result = engine.Execute(report, EnterpriseScenarioCatalog.CreateOperationalRows(Now));

        Assert.HasCount(3, result.Rows);
        Assert.IsTrue(result.Rows.All(static row => row.Values.ContainsKey("ageHours")));
        Assert.IsEmpty(result.Diagnostics);
    }
}
