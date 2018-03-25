namespace Sample
{
    public partial class PartialClass
    {
        private readonly int value;

        public PartialClass(int value)
        {
            this.value = value;
        }

        partial void APartialMethod();

        public void CallPartialMethod()
        {
            APartialMethod();
        }
    }
}