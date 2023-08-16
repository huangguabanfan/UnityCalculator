public abstract class OperatorFactory
{
    public abstract OperatorBase CreateOperator();
}

public class OperatorAdditionFactory : OperatorFactory
{
    public override OperatorBase CreateOperator()
    {
        return new OperatorAddition();
    }
}

public class OperatorSubTractFactory : OperatorFactory
{
    public override OperatorBase CreateOperator()
    {
        return new OperatorSubtract();
    }
}

public class OperatorMultiplyFactory : OperatorFactory
{
    public override OperatorBase CreateOperator()
    {
        return new OperatorMultiply();
    }
}

public class OperatorDivisionFactory : OperatorFactory
{
    public override OperatorBase CreateOperator()
    {
        return new OperatorDivision();
    }
}