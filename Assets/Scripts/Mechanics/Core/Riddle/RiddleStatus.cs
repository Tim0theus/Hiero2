public class RiddleStatus {
    public bool Solved { get; private set; }
    public bool Failure { get; private set; }
    public RiddleStatus(bool solved, bool failure) {
        Solved = solved;
        Failure = failure;
    }
}

