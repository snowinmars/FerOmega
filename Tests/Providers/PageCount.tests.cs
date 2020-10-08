using NUnit.Framework;

namespace FerOmega.Tests.Providers
{
    internal class PageCountTests : BaseTest
    {
        [Test]
        public void PageCount()
        {
            const string equation = "left + right";
            const string expectedSql = "where left + right limit 2 offset 2";
            
            var tokens = TokenizationService.Tokenizate(equation);
            var tree = AstService.Convert(tokens);
            var (where, parameters) = SqlProvider.Convert(tree,
                                                          SqlProvider.DefineProperty().From("left").ToSql("left"),
                                                          SqlProvider.DefineProperty().From("right").ToSql("right"));

            where.AddPage(1).AddCount(2);
            
            Assert.AreEqual(expectedSql, where.ToString());

            Assert.AreEqual(0, parameters.Length);
        }
        
        [Test]
        public void Count()
        {
            const string equation = "left + right";
            const string expectedSql = "where left + right limit 2";
            
            var tokens = TokenizationService.Tokenizate(equation);
            var tree = AstService.Convert(tokens);
            var (where, parameters) = SqlProvider.Convert(tree,
                                                          SqlProvider.DefineProperty().From("left").ToSql("left"),
                                                          SqlProvider.DefineProperty().From("right").ToSql("right"));

            where.AddCount(2);
            
            Assert.AreEqual(expectedSql, where.ToString());

            Assert.AreEqual(0, parameters.Length);
        }
        
        [Test]
        public void Page()
        {
            const string equation = "left + right";
            const string expectedSql = "where left + right offset 2";
            
            var tokens = TokenizationService.Tokenizate(equation);
            var tree = AstService.Convert(tokens);
            var (where, parameters) = SqlProvider.Convert(tree,
                                                          SqlProvider.DefineProperty().From("left").ToSql("left"),
                                                          SqlProvider.DefineProperty().From("right").ToSql("right"));

            where.AddPage(1);
            
            Assert.AreEqual(expectedSql, where.ToString());

            Assert.AreEqual(0, parameters.Length);
        }
    }
}