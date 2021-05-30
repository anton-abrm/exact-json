using System;

namespace ExactJson
{
    public abstract class JsonReaderState
    {
        protected JsonReaderState(JsonReader reader)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }
        
        public JsonReader Reader { get; }

        public abstract void Restore();
    }
}