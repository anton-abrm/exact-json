// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ExactJson
{
    public abstract partial class JsonNode
    {
        private readonly struct PointerSection
        {
            public PointerSection(string name)
            {
                _name = name;
                _index = 0;
            }

            public PointerSection(int index)
            {
                _name = null;
                _index = index;
            }

            private readonly string _name;
            private readonly int _index;

            public JsonPointer AttachTo(JsonPointer pointer)
            {
                return _name is not null
                    ? pointer.Attach(_name)
                    : pointer.Attach(_index);
            }
        }
    }
}