using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [RegisterModule("Modules/Simple Outline")]
    public class SimpleOutlineInitModule : InitModule
    {
        [SerializeField] Material fillMaterial;
        [SerializeField] Material maskMaterial;

        public override void CreateComponent(Initialiser Initialiser)
        {
            SimpleOutline.ApplyMaterials(fillMaterial, maskMaterial);
        }

        public SimpleOutlineInitModule()
        {
            moduleName = "Simple Outline";
        }
    }
}