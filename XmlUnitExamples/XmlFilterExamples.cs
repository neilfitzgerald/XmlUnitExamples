using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.XmlUnit.Builder;

namespace XmlUnitExamples
{
    [TestClass]
    public class XmlFilterExamples
    {
        [TestMethod]
        public void TestDemoFilteringAttributes()
        {
            string inputOne = "<x><something fluff='a'/><a stuff='a'/><b stuff='b'/></x>";
            string inputtwo = "<x><something fluff='b'/><a stuff='b'/><b stuff='c'/></x>";
            var diff = DiffBuilder.Compare(inputOne)
                .WithTest(inputtwo)
                .IgnoreWhitespace()
                .WithAttributeFilter(
                    attribute =>
                        {
                            return
                                // Filter attribute out in all cases
                                !attribute.Name.Equals("fluff", StringComparison.CurrentCultureIgnoreCase)
                                // Filter attribute out when the owner node is a specific tag type
                                && !(attribute.Name.Equals("stuff", StringComparison.CurrentCultureIgnoreCase)
                                    && attribute.OwnerElement.Name.Equals("a", StringComparison.CurrentCultureIgnoreCase));
                        }).Build();
            Assert.IsTrue(diff.HasDifferences());
            int count = 0;
            foreach (var difference in diff.Differences)
            {
                Console.WriteLine(difference);
                Assert.AreEqual(@"Expected attribute value 'b' but was 'c' - comparing <b stuff=""b""...> at /x[1]/b[1]/@stuff to <b stuff=""c""...> at /x[1]/b[1]/@stuff (DIFFERENT)", difference.ToString());
                count++;
            }
            Assert.AreEqual(1, count, "Only one diff expected");
        }

        [TestMethod]
        public void TestDemoFilteringElements()
        {
            string inputOne = "<x><something fluff='a'/><something>a</something><a stuff='a'/><b stuff='b'/></x>";
            string inputtwo = "<x><something fluff='b'/><something>b</something><a stuff='b'/><b stuff='c'/></x>";
            var diff = DiffBuilder.Compare(inputOne)
                .WithTest(inputtwo)
                .IgnoreWhitespace()
                .WithNodeFilter(
                    node =>
                    {
                        return
                            // Filter out an element in all casses
                            !(node.NodeType.Equals(System.Xml.XmlNodeType.Element) && node.Name.Equals("a", StringComparison.CurrentCultureIgnoreCase))
                            // Filter out an element if it has attributes
                            && !(node.NodeType.Equals(System.Xml.XmlNodeType.Element)
                                && node.Name.Equals("something", StringComparison.CurrentCultureIgnoreCase)
                                && node.Attributes.Count > 0);
                    }).Build();
            Assert.IsTrue(diff.HasDifferences());
            int count = 0;
            foreach (var difference in diff.Differences)
            {
                Console.WriteLine(difference);
                count++;
            }
            Assert.AreEqual(2, count, "Two diffs expected");
        }
    }
}