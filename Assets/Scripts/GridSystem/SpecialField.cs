using System;
using System.Collections.Generic;
using UnityEngine;

public class SpecialField
    {
        public SpecialFieldType FieldType;
        public List<GridTile> SpecialFieldGridTiles = new();
        private bool alreadyFulfilled = false;

        public SpecialField(SpecialFieldType fieldType)
        {
            FieldType = fieldType;
        }


        public bool IsFulfilled()
        {
            bool fulfilled = true;
            foreach (GridTile gridTile in SpecialFieldGridTiles)
            {
                if (!gridTile.ContainsPlant())
                {
                    fulfilled = false;
                    break;
                }
            }

            return fulfilled;
        }

        public void TryExecuteFunction()
        {
            if (!IsFulfilled()) return;
            if (alreadyFulfilled) return;

            switch (FieldType)
            {
                case SpecialFieldType.SHOP:
                    break;
                case SpecialFieldType.CARD_REMOVE:
                    break;
                case SpecialFieldType.CARD_ADD:
                    break;
                case SpecialFieldType.RETRIGGER:
                    break;
                case SpecialFieldType.DUPLICATE:
                    break;
                case SpecialFieldType.MANA:
                    break;
                case SpecialFieldType.ESSENCE:
                    break;
                case SpecialFieldType.UNLOCK_PLANT:
                    break;
                case SpecialFieldType.HALF_ECO:
                    break;
                case SpecialFieldType.TIME_PLAY:
                    break;
                case SpecialFieldType.MULTIPLY:
                    foreach (var specialFieldGridTile in SpecialFieldGridTiles)
                    {
                        CallerArgs multiplyFieldCallerArgs = new CallerArgs()
                        {
                            callerType = CALLER_TYPE.EFFECT,
                            playedTile = specialFieldGridTile
                        };
                        GameManager.Instance.AddPointScore(
                            Mathf.FloorToInt(specialFieldGridTile.CardInstance.CardData.RuntimePoints *
                                             Constants.MULTIPLICATION_FIELD_MULTIPLIER), multiplyFieldCallerArgs,
                            GameManager.SCORING_ORIGIN.MULTIPLICATION);
                    }

                    break;
                case SpecialFieldType.NORMAL_FIELD:
                    break;
                case SpecialFieldType.NONE:
                    break;
                case SpecialFieldType.CENTER:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            alreadyFulfilled = true;
        }
    }