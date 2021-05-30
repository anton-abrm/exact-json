using Moq;
using Moq.Protected;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public sealed class JsonWriterTests
    {
        #region Dispose

        [Test]
        public void Dispose_ByDefault_DisposeWithDisposingAsTrueCalled()
        {
            var mock = new Mock<JsonWriter>();

            mock.Object.Dispose();

            mock.Protected().Verify("Dispose", Times.Once(), true, true);
            mock.VerifyNoOtherCalls();
        }

        #endregion
    }
}