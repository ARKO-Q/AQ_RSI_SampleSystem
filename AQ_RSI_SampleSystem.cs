#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

/***************************************************************************************
  AAAAA   RRRRR    K   K   OOO      QQQQQ   U   U   AAAAA   N   N  TTTTT  U   U  M   M  
 A     A  R    R   K  K   O   O    Q    Q   U   U  A     A  NN  N    T    U   U  MM MM  
 AAAAAAA  RRRRR    KKK    O   O    Q  Q Q   U   U  AAAAAAA  N N N    T    U   U  M M M  
 A     A  R   R    K  K   O   O    Q   QQ   U   U  A     A  N  NN    T    U   U  M   M  
 A     A  R    R   K   K   OOO     QQQQQ Q  UUUU   A     A  N   N    T     UUUU  M   M  
 **************************************************************************************/

/******************************************************************************
 * File	            :   AQ-RSI-SampleSystem.cs                                       
 * Author	    :   ARKO Quantum S.R.L.                                              
 * Created          :   04/03/2025                                                         
 * Updated	    :	07/03/2025                                                            
 * ----------------------------------------------------------------------------            
 * Email            :   contacto@arkoquantum.com                                              
 * Website          :   arkoquantum.com                                                        
 *                                                                             
 * Software licensed under the terms of the GNU GPL v3.0 open source license.                            
 *****************************************************************************/

/******************************************************************************
 * Description:
 *  This RSI-based trading strategy enters a long position when the RSI drops below X,
 *  signaling potential oversold conditions, and a short position when the RSI rises above Y,
 *  indicating potential overbought conditions. Stop-loss orders are set using a multiple
 *  of the ATR (Average True Range) to adapt to market volatility. 
 *  
 *  - Optimizable:
 *  --> RSI Period
 *  --> ATR Multiplier               
 *****************************************************************************/

namespace NinjaTrader.NinjaScript.Strategies
{
	public class AQALGO002 : Strategy
	{

		#region Variables
		private RSI rsi;
        #endregion

        #region Settings
        protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description								= @"";
				Name									= "AQ-RSI-SampleSystem";
				Calculate								= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IncludeCommission							= true;
                		IsExitOnSessionCloseStrategy						= false;
				ExitOnSessionCloseSeconds						= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage								= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce								= TimeInForce.Gtc;
				TraceOrders								= false;
				RealtimeErrorHandling							= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 5;
                		// Disable this property for performance gains in Strategy Analyzer optimizations
               			// See the Help Guide for additional information
                		IsInstantiatedOnEachOptimizationIteration				= false;

				// Paremeters
				RSI_Period								= 14;
				ATR_Multiplier								= 1;
            }

	    else if (State == State.Configure)
	    {
            }

            else if (State == State.DataLoaded)
            {
		rsi = RSI(RSI_Period, 1);
		AddChartIndicator(rsi);

                ClearOutputWindow(); 
            }
        }
        #endregion

        #region System Logic
        protected override void OnBarUpdate()
	{
	    if (CurrentBar < BarsRequiredToTrade) return;

	    if (rsi[1] < 30 && rsi[0] > 30)
	    {
		SetStopLoss(CalculationMode.Price, Close[0] - (ATR_Multiplier * ATR(10)[0]));
		EnterLong();
	    }
			
            if (rsi[1] > 70 && rsi[0] < 70)
            {
		SetStopLoss(CalculationMode.Price, Close[0] + (ATR_Multiplier * ATR(10)[0]));
                EnterShort();
            }
        }
        #endregion

        #region Properties
        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "RSI Period", Order = 1, GroupName = "Parameters")]
        public int RSI_Period { get; set; }

        [NinjaScriptProperty]
        [Range(0.1, double.MaxValue)]
        [Display(Name = "ATR Multiplier", Order = 2, GroupName = "Parameters")]
        public double ATR_Multiplier { get; set; }
        #endregion
    }
}
