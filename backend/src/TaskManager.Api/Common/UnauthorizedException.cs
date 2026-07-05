namespace TaskManager.Api.Common;

public class UnauthorizedException(string message) : Exception(message);
