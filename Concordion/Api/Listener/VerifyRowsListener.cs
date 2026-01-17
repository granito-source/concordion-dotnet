namespace Concordion.Api.Listener;

public interface VerifyRowsListener {
    void ExpressionEvaluated(ExpressionEvaluatedEvent expressionEvaluatedEvent);

    void MissingRow(MissingRowEvent missingRowEvent);

    void SurplusRow(SurplusRowEvent surplusRowEvent);
}
