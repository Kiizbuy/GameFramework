﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Inventory.UI
{
    public class FakeChangeInventoryOwner : MonoBehaviour
    {
        public InventoryUIView InventoryUIView;
        public Inventory Inventory;

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
                InventoryUIView.ChangeInventoryOwner(Inventory);
        }
    }
}
