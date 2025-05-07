namespace DaimlerConfigTest
{
    public class UnitTest1
    {
        

        [Fact]
        public void Addition_ReturnsCorrectSum()
        {
            // Arrange
            int a = 5;
            int b = 3;

            // Act
            int result = a + b;

            // Assert
            Assert.Equal(8, result);
        }


    }
}