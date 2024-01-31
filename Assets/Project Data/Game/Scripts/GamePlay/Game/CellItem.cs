using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon.JellyMerge
{
    [System.Serializable]
    public class CellItem
    {
        [SerializeField]
        private ColorId colorID;
        public ColorId ColorID
        {
            get { return colorID; }
        }

        [SerializeField]
        private CellBehaviour cellBehaviour;
        public CellBehaviour Cell
        {
            get { return cellBehaviour; }
        }

        public CellItem()
        {
            colorID = ColorId.None;
            cellBehaviour = null;
        }

        public void InitEmptyItem()
        {
            colorID = ColorId.None;
            cellBehaviour = null;
        }

        public void InitStaticItem()
        {
            colorID = ColorId.Static;
            cellBehaviour = null;
        }

        public void InitColoredItem(CellBehaviour cell)
        {
            cellBehaviour = cell;
            colorID = cell.ColorID;
        }
    }
}