namespace TRXToHTMLConvert
{
    /// <summary>
    /// UnitTestResult
    /// </summary>
    public class TheTestResultElement
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TheTestResultElement()
        {
        }

        /// <summary>
        /// Constructor from XElement
        /// </summary>
        /// <param name="elem"></param>
        public TheTestResultElement(XElement elem)
        {
            TestId = elem.Attribute("testId").Value;
            ExecutionId = elem.Attribute("executionId").Value;
            Duration = elem.Attribute("duration").Value;
            Outcome = elem.Attribute("outcome").Value;
            TestName = elem.Attribute("testName").Value;

            string results = string.Join(" ", [.. elem.DescendantNodes().Select(x => x.ToString())]);
            TestBody = results.Split(">\r\n")[1];
            TestBody = TestBody.Replace("-&gt;", "").Replace("</StdOut", "")
                .Replace("<StdOut>", "").Replace("+&lt;", "");

        }

        /// <summary>
        /// testBody
        /// </summary>
        public string TestBody { get; set; }

        /// <summary>
        /// testId
        /// </summary>
        public string TestId { get; set; }

        /// <summary>
        /// executionId
        /// </summary>
        public string ExecutionId { get; set; }

        /// <summary>
        /// duration
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// outcome
        /// </summary>
        public string Outcome { get; set; }

        /// <summary>
        /// testName
        /// <summary>
        public string TestName { get; set; }
    }

    /// <summary>
    /// UnitTest
    /// </summary>
    public class TheUnitTestElement
    {

        public TheUnitTestElement()
        {

        }

        /// <summary>
        /// Initializes a new instance of the UnitTestElement class using the specified XML element.
        /// </summary>
        /// <param name="elem">The XML element containing the unit test data. Must not be null and is expected to have 'id' and 'name'
        /// TestMethod is optional
        /// attributes.</param>
        public TheUnitTestElement(XElement elem)
        {
            Id = elem.Attribute("id").Value;
            Name = elem.Attribute("name").Value;
            var testMethodElem = elem.Elements().Where(x => x.Name.LocalName == "TestMethod").FirstOrDefault();
            if (testMethodElem != null)
            {
                TestMethod = new TheTestMethodElement(testMethodElem);
            }
        }


        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// TestMethod
        /// </summary>
        public TheTestMethodElement TestMethod { get; set; }
    }

    /// <summary>
    /// TestMethod
    /// </summary>
    public class TheTestMethodElement
    {
        public TheTestMethodElement()
        {

        }

        public TheTestMethodElement(XElement elem)
        {
            ClassName = elem.Attribute("className").Value;
            Name = elem.Attribute("name").Value;
        }

        /// <summary>
        /// className
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// name
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// ResultSummary
    /// </summary>
    public class TheResultSummaryElement
    {
        public TheResultSummaryElement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResultSummaryElement class using the specified XML element.
        /// </summary>
        /// <param name="elem"></param>
        public TheResultSummaryElement(XElement elem)
        {
            var countersElem = elem.Elements().Where(x => x.Name.LocalName == "Counters").FirstOrDefault();
            if (countersElem != null)
            {
                Counters = new CountersElement(countersElem);
            }
        }
        public CountersElement Counters { get; set; }
    }

    /// <summary>
    /// StdOut
    /// </summary>
    public class StdOutElement
    {
        public StdOutElement()
        {
        }

        public StdOutElement(XElement elem)
        {
            StdText = elem.Value;
        }

        public string StdText { get; set; }
        public string TestId { get; set; }
    }

    /// <summary>
    /// Counters
    /// </summary>
    public class CountersElement
    {
        public CountersElement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CountersElement class using the specified XML element.
        /// </summary>
        /// <param name="elem"></param>
        public CountersElement(XElement elem)
        {
            Total = elem.Attribute("total")?.Value;
            Executed = elem.Attribute("executed")?.Value;
            Passed = elem.Attribute("passed")?.Value;
            Failed = elem.Attribute("failed")?.Value;
            Error = elem.Attribute("error")?.Value;
            Timeout = elem.Attribute("timeout")?.Value;
            Aborted = elem.Attribute("aborted")?.Value;
            Inconclusive = elem.Attribute("inconclusive")?.Value;
            PassedButRunAborted = elem.Attribute("passedButRunAborted")?.Value;
            NotRunnable = elem.Attribute("notRunnable")?.Value;
            NotExecuted = elem.Attribute("notExecuted")?.Value;
            Disconnected = elem.Attribute("disconnected")?.Value;
            Warning = elem.Attribute("warning")?.Value;
            Completed = elem.Attribute("completed")?.Value;
            InProgress = elem.Attribute("inProgress")?.Value;
            Pending = elem.Attribute("pending")?.Value;
        }

        public string Total { get; set; }
        public string Executed { get; set; }
        public string Passed { get; set; }
        public string Failed { get; set; }
        public string Error { get; set; }
        public string Timeout { get; set; }
        public string Aborted { get; set; }
        public string Inconclusive { get; set; }
        public string PassedButRunAborted { get; set; }
        public string NotRunnable { get; set; }
        public string NotExecuted { get; set; }
        public string Disconnected { get; set; }
        public string Warning { get; set; }
        public string Completed { get; set; }
        public string InProgress { get; set; }
        public string Pending { get; set; }

    }
}
