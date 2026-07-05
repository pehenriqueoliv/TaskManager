namespace TaskManager.Api.Common;

public class NotFoundException(string message) : Exception(message);
