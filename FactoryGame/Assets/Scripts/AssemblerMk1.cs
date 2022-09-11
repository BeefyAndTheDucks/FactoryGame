using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblerMk1 : Machine
{
    string type = "assembler1";
    public InputControl[] inputs = new InputControl[1];
    public Belt[] outputBelts = new Belt[1];
    public Recipe recipe;
}
