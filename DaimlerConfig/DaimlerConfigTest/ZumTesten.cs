using Xunit; // Wichtig für xUnit
using DaimlerConfig.Components.ZumTesten; // Namespace der zu testenden Klasse

namespace DaimlerConfigTest
{
    public class ZumTestenTests // Testklassen sollten oft mit "Tests" enden
    {
        [Fact]
        public void ZumTest_Works()
        {
            // Arrange
            var zum = new ZumTest(); // Instanziiere die zu testende Klasse
            int a = 2;
            int b = 3;

            // Act
            int result = zum.addNum(a, b);

            // Assert
            Assert.Equal(5, result); // Überprüfe das Ergebnis
        }
    }
}