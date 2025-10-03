using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using GameCreator.Runtime.VisualScripting;

[Title("Get highest value")]
[Description("Get highest value + index in a list")]
[Version(1, 1, 2)]

[Category("Math/Arithmetic/Highest List Value")]

[Parameter("List", "List source")]
[Parameter("Index", "The index of the highest value")]
[Parameter("Value", "The highest value")]

[Keywords("Highest", "List", "Value", "Variable")]
[Image(typeof(IconPlusCircle), ColorTheme.Type.Blue, typeof(OverlayDot))]

[Serializable]
public class InstructionGetMaxElement : Instruction
{
    // MEMBERS: -------------------------------------------------------------------------------
    [SerializeField] private CollectorListVariable variableList;
    [SerializeField] private PropertySetNumber m_Index = SetNumberGlobalName.Create;
    [SerializeField] private PropertySetNumber m_Val = SetNumberGlobalName.Create;

    private List<object> numbers;

    // PROPERTIES: ----------------------------------------------------------------------------
    public override string Title => $"Highest Value in: {this.variableList}";

    // RUN METHOD: ----------------------------------------------------------------------------
    protected override Task Run(Args args){
        numbers = variableList.Get(args);
        double value = numbers.Select(x => ConvertToDouble(x)).Max();
        double index = numbers.IndexOf(numbers.Max());

        this.m_Index.Set(index, args);
        this.m_Val.Set(value, args);

        return DefaultResult;
    }

    // FUNCTIONS : ----------------------------------------------------------------------------
    private double ConvertToDouble(object obj)
    {
        if (obj == null || !double.TryParse(obj.ToString(), out double result))
            return 0; // Default value when conversion fails
        return result;
    }
}