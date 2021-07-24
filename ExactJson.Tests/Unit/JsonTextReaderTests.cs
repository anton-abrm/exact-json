// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public sealed class JsonTextReaderTests
    {
        #region Dispose
        
        [Test]
        public void Dispose_WithCloseInput_ClosesInternalReader()
        {
            var tr = new StringReader("[]");
        
            var jr = new JsonTextReader(tr)
            {
                CloseInput = true
            };
        
            jr.Dispose();

            Assert.Throws<ObjectDisposedException>(() => tr.ReadToEnd());
        }
        
        [Test]
        public void Dispose_WithoutCloseInput_DoesNotCloseInternalReader()
        {
            var tr = new StringReader("[]");
        
            var jr = new JsonTextReader(tr)
            {
                CloseInput = false
            };
        
            jr.Dispose();

            Assert.That(tr.Read(), Is.EqualTo((int) '['));
        }

        #endregion
    }
}