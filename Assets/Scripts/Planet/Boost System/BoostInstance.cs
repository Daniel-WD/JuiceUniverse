public class BoostInstance {
    
    public Boost boost;
    
    public long activeTime = -1;
    public long activeTimeLeft = -1;
    
    public BoostInstance(Boost b, long activeTime) {
        boost = b;
        this.activeTime = activeTime;
    }
    
    public BoostInstance() {}
    
}