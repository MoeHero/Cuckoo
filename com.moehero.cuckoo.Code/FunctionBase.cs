namespace com.moehero.cuckoo.Code
{
    internal abstract class FunctionBase
    {
        public virtual string Description { get; } = "";

        public virtual bool Handled { get; set; } = true;

        public virtual bool CanRun() => true;

        public abstract void Run();
    }
}
