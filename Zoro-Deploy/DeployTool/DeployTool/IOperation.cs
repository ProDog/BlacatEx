namespace DeployTool
{
    public interface IOperation
    {
        string Name { get; }
        string ID { get; }

        void Start();
    }
}
