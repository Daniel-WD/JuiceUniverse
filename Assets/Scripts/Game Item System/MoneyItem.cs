using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

[CreateAssetMenu(fileName = "New MoneyItem", menuName = "MoneyItem")]
public class MoneyItem : GameItem {
    
    public int mantissa, exponent;
    
    private BigDecimal mAmount;
    
    public override string info() {
        calcAmount();
        return Formatter.formatNumber(mAmount);
    }

    public override void makeAvailable() {
        calcAmount();
        Database.inst.addPrimaryMoney(mAmount);
    }
    
    private void calcAmount() {
        mAmount = new BigDecimal(mantissa, exponent);
    }
    
}
