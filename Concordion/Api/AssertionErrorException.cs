namespace Concordion.Api;

/// <summary>
/// Signals that a specification has not processed properly
/// </summary>
public class AssertionErrorException : Exception
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionErrorException"/> class.
    /// </summary>
    public AssertionErrorException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionErrorException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public AssertionErrorException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionErrorException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner.</param>
    public AssertionErrorException(string message, Exception inner)
        : base(message, inner)
    {
    }

    #endregion
}
