namespace TaskManager.Api.Common;

public class BadRequestException(string message) : Exception(message);
