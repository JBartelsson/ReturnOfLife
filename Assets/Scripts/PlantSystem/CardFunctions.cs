// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public static class CardFunctions
// {
//     public static void ExecuteCard(CallerArgs callerArgs)
//     {
//         switch (callerArgs.CallingCardInstance.CardData.LifeformType)
//         {
//             case CardData.LifeformTypeEnum.Antisocial:
//                 Function_Antisocial(callerArgs);
//                 break;
//             case CardData.LifeformTypeEnum.Bindweed:
//                 break;
//             case CardData.LifeformTypeEnum.Lycoperdon:
//                 break;
//             case CardData.LifeformTypeEnum.Epiphyt:
//                 break;
//             case CardData.LifeformTypeEnum.Normalo:
//                 break;
//             case CardData.LifeformTypeEnum.Reanimate:
//                 break;
//             case CardData.LifeformTypeEnum.Epiphany:
//                 break;
//             case CardData.LifeformTypeEnum.Pairy:
//                 break;
//             default: 
//         }
//     }
//
//     private static void Function_Antisocial(CallerArgs callerArgs)
//     {
//         GridTile gridTile = callerArgs.playedTile;
//         gridTile.AddObject(callerArgs);
//         bool hasPlantInPattern = false;
//         Debug.Log($"====================================");
//         gridTile.ForPattern(callerArgs.CallingCardInstance.GetCardStats().EffectPattern, tile =>
//         {
//             if (tile.CardInstance == null) return;
//             if (tile.CardInstance.IsDead()) return;
//             Debug.Log(tile.CardInstance.CardData.CardName);
//             Debug.Log(callerArgs.CallingCardInstance.CardData.CardName);
//
//             
//             if (tile.CardInstance.CardData.CardName == callerArgs.CallingCardInstance.CardData.CardName)
//             {
//                 Debug.Log($"HAS PLANT IN PATTERN!!!");
//                 hasPlantInPattern = true;
//             }
//         } );
//         Debug.Log($"Checking if return is working");
//         CardData callingCardData = callerArgs.CallingCardInstance.CardData;
//         if (hasPlantInPattern)
//         {
//             Debug.Log($"OVERRIDING PLANT FUNCTION!");
//             callingCardData.RuntimeScore = new Score(0);
//             callingCardData.OverridePointFunction = true;
//         }
//     }
//
//     private static void Function_Bindweed(CallerArgs callerArgs)
//     {
//         
//     }
//     
// }
