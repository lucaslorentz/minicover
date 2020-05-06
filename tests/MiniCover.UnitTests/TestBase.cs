using System;
using Moq;

namespace MiniCover.UnitTests
{
    public class TestBase : IDisposable
    {
        private readonly MockRepository _mockRepository;

        public TestBase()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
        }

        public Mock<T> MockFor<T>()
            where T : class
        {
            return _mockRepository.Create<T>();
        }

        public void Dispose()
        {
            _mockRepository.VerifyAll();
        }
    }
}
