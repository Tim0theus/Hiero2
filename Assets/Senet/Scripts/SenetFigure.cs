using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class SenetFigure : PickUp {

        public bool flat = false;

        private SenetMain senet;

        private new void Awake()
        {
            base.Awake();
            senet = GetComponentInParent<SenetMain>();
        }

        private void Start()
        {
            
        }

        protected override void Pickup()
        {
            senet.OnPickup(this);
            base.Pickup();
        }

        public override void PutDown()
        {
            base.PutDown();
        }

        public override void Drop()
        {
            base.Drop();
        }

        public void ActivateFigure()
        {
            ActivatePickup();
            Fader.Activate();
        }

        public void DeactivateFigure()
        {
            DeactivatePickup();
            Fader.DeActivate();
        }
}
