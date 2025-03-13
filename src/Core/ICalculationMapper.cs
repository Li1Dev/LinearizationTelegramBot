namespace Lin.Core;

public interface ICalculationMapper
{
    public string MapToText(CalculateResult calculateResult);

    DrowRequest MapToDrowRequest(CalculateResult calculateResult);
}