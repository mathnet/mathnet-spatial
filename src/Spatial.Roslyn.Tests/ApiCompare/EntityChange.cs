using System;
using System.Collections.Generic;
using System.Text;

namespace Spatial.Roslyn.Tests.ApiCompare
{
    public class EntityChange
    {
        public Type OriginalType { get; set; }

        public Type ReplacementType { get; set; }

        public List<string> MethodChanges { get; set; } = new List<string>();

        public bool IsChanged
        {
            get { return this.IsRemoved || this.MethodChanges.Count > 0; }
        }

        public bool IsRemoved
        {
            get
            {
                if (this.ReplacementType == null || this.ReplacementType.IsDefinedReflectOnly(typeof(ObsoleteAttribute)))
                {
                    return true;
                }

                return false;
            }
        }

        internal string GetTransitionString()
        {
            StringBuilder sb = new StringBuilder();
            if (this.ReplacementType == null)
            {
                sb.Append("[Type Deleted]");
            }
            else if (this.ReplacementType.IsDefinedReflectOnly(typeof(ObsoleteAttribute)))
            {
                sb.Append("[Type Obsoleted]");
            }
            else
            {
                if (this.OriginalType.IsPublic)
                {
                    if (!this.ReplacementType.IsPublic)
                    {
                        sb.Append("[No Longer Public]");
                    }
                }
            }

            return sb.ToString();
        }
    }
}
