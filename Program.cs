namespace Trxlog2Html
{
    class Program : ConsoleAppBase
    {
        /// <summary>
        /// Entry point of the application
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                //for testing purpose when debugging
                args = new string[6];
                args[0] = "-i";
                args[1] = "C:\\Files\\testResults.trx";
                args[2] = "-o";
                args[3] = "C:\\Files\\testResults.html";
                args[4] = "-t";
                args[5] = "";
            }
            await Host.CreateDefaultBuilder().RunConsoleAppFrameworkAsync<Program>(args);
        }


        /// <summary>
        /// Convert trx log file to HTML report.
        /// </summary>
        /// <param name="fileIn"></param>
        /// <param name="fileOut"></param>
        /// <param name="templateHTMLFilePath"></param>
        public void All(
            [Option("i", "Input file path (required).")] string inputFilePath,
            [Option("o", "Output file path (required).")] string outputFilePath,
            [Option("t", "Template file path (optional).")] string templateHTMLFilePath = null)
        {
            if (templateHTMLFilePath == null || templateHTMLFilePath == "")
            {
                // if no template is specified then use built-in template
                templateHTMLFilePath = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "HTMLTemplate"), "template.cshtml");
            }
            var template = ReadTheHTMLTemplate(templateHTMLFilePath);
            var model = CreateTheModelFromXml(inputFilePath);
            var html = Engine.Razor.RunCompile(template, "templateKey", typeof(TheTestReportModel), model);
            WriteTheResultToFile(outputFilePath, html);
        }

        /// <summary>
        /// read the HTML template from file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string ReadTheHTMLTemplate(string path)
        {
            using var reader = new StreamReader(path, System.Text.Encoding.UTF8);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// write the result to output file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="result"></param>
        private static void WriteTheResultToFile(string path, string result)
        {
            using var writer = new StreamWriter(path, false, System.Text.Encoding.UTF8);
            writer.Write(result);
        }
        
        /// <summary>
        /// create report models for razor engine from the trx log file.
        /// </summary>
        /// <param name="xmlPath">path of the trx log file</param>
        /// <returns></returns>
        private TheTestReportModel CreateTheModelFromXml(string xmlFilePath)
        {
            var whatsUpDoc = XDocument.Load(xmlFilePath);
            var unitTestClasses = whatsUpDoc.Descendants()
                .Where(x => x.Name.LocalName == "UnitTest")
                .Select(x => new TheUnitTestElement(x))
                .Where(x => x.TestMethod != null)
                .GroupBy(x => x.TestMethod.ClassName)
                .OrderBy(x => x.Key)
                .ToList();

            var unitTestResults = whatsUpDoc.Descendants()
                .Where(x => x.Name.LocalName == "UnitTestResult")
                .Select(x => new TheTestResultElement(x))
                .GroupBy(x => x.TestId)
                .ToDictionary(x => x.Key, x => x.ToList());

            var resultSummary = whatsUpDoc.Descendants().
                Where(x => x.Name.LocalName == "ResultSummary")
                .Select(x => new TheResultSummaryElement(x))
                .FirstOrDefault();

            var startTime = whatsUpDoc.Descendants().
                Where(x => x.Name.LocalName == "Times")
                .Select(x => x.Attribute("start").Value)
                .FirstOrDefault();

            var model = new TheTestReportModel
            {
                Summary = Map(resultSummary),
                TestClasses = unitTestClasses.Select(x => Map(x, unitTestResults)).ToList(),
                StartTime = startTime
            };
            return model;
        }

        /// <summary>
        /// map TheUnitTestElement group to TheTestReportClassModel
        /// </summary>
        /// <param name="src"></param>
        /// <param name="testResults"></param>
        /// <returns></returns>
        private TheTestReportClassModel Map(
            IGrouping<string, TheUnitTestElement> src,
            Dictionary<string, List<TheTestResultElement>> testResults)
        {
            var ret = new TheTestReportClassModel();
            ret.ClassName = src.Key;

            ret.TestResults = [.. src
                .SelectMany(x => Map(x, testResults))
                .Where(x => x != null)
                .OrderBy(x => x.TestMethod)];
            return ret;
        }

        /// <summary>
        /// map TheUnitTestElement to TestReportResultModel collection
        /// </summary>
        /// <param name="src"></param>
        /// <param name="testResults"></param>
        /// <returns></returns>
        private IEnumerable<TestReportResultModel> Map(
            TheUnitTestElement src,
            Dictionary<string, List<TheTestResultElement>> testResults)
        {
            if (testResults.TryGetValue(src.Id, out var testResult))
            {
                foreach (var result in testResult)
                {
                    yield return new TestReportResultModel
                    {
                        DisplayName = result.TestName,
                        TestMethod = src.TestMethod.Name,
                        Duration = result.Duration,
                        Outcome = result.Outcome,
                        TestBody = result.TestBody
                    };
                }
            }
        }

        /// <summary>
        /// map TheResultSummaryElement to TestReportSummaryModel
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private TestReportSummaryModel Map(TheResultSummaryElement src)
        {
            var ret = new TestReportSummaryModel()
            {
                Aborted = src.Counters.Aborted,
                Completed = src.Counters.Completed,
                Disconnected = src.Counters.Disconnected,
                Error = src.Counters.Error,
                Executed = src.Counters.Executed,
                Failed = src.Counters.Failed,
                Inconclusive = src.Counters.Inconclusive,
                InProgress = src.Counters.InProgress,
                NotExecuted = src.Counters.NotExecuted,
                NotRunnable = src.Counters.NotRunnable,
                Passed = src.Counters.Passed,
                PassedButRunAborted = src.Counters.PassedButRunAborted,
                Pending = src.Counters.Pending,
                Timeout = src.Counters.Timeout,
                Total = src.Counters.Total,
                Warning = src.Counters.Warning,
            };
            return ret;
        }
    }
}
