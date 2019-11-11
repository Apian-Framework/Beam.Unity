using BeamBackend;

public interface IBikeControl
{
    void Setup(IBike beBike, IBeamBackend backend);   
    void Loop(float frameSecs);
}

public abstract class BikeControl : IBikeControl
{
    protected IBike ib;
    protected IBeamBackend be;

    public BikeControl()  { }

    public void Setup(IBike ibike, IBeamBackend backend)
    {
        ib = ibike;
        be = backend;
        SetupImpl();
    }

    public abstract void SetupImpl(); // do any implmentation-specific setup

    public abstract void Loop(float frameSecs);

}