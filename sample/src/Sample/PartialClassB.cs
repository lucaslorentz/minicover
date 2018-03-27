using System;

namespace Sample
{
    public partial class PartialClass
    {
        partial void APartialMethod()
        {
            throw new Exception(message: value.ToString());
        }
    }
}