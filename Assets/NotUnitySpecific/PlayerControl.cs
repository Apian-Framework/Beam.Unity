using BeamBackend;

public class PlayerControl : BikeControl
{
    public override void SetupImpl() 
    {
        
    }

    public override void Loop(float frameSecs)
    {
    }  

    public void OnPlayerTurnRequest(TurnDir dir)
    {
        be.OnTurnReq(ib.bikeId, dir);        
    }

}
