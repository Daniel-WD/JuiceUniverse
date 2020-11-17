public class PerkInstance {
    
    public Perk perk;
    
    public long activeTimeLeft = -1;
    public long activeTime = 60;

    public long sleepTimeLeft = -1;
    public long sleepTime = 100;
    
    public PerkInstance(Perk p, long activeTime, long sleepTime) {
        perk = p;
        this.activeTime = activeTime;
        this.sleepTime = sleepTime;
    }
    
    public PerkInstance() {}
    
}