using System.Net;

namespace Application.Results
{
    /// <summary>
    /// Contains pagination metadata for a paginated result.
    /// </summary>
    public class PaginationInfo
    {
        /// <summary>The current page number (1-based).</summary>
        public int CurrentPage { get; init; }
        /// <summary>The number of items per page.</summary>
        public int PageSize { get; init; }
        /// <summary>The total number of items across all pages.</summary>
        public int TotalCount { get; init; }
        /// <summary>The total number of pages based on <see cref="TotalCount"/> and <see cref="PageSize"/>.</summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        /// <summary><see langword="true"/> if there is a page before the current one.</summary>
        public bool HasPreviousPage => CurrentPage > 1;
        /// <summary><see langword="true"/> if there is a page after the current one.</summary>
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    /// <summary>
    /// Contains sorting metadata returned alongside a list result.
    /// </summary>
    public class SortInfo
    {
        /// <summary>The list of column names available for sorting.</summary>
        public required List<string> SortColumns { get; set; }
        /// <summary>The supported sort directions (e.g. "asc", "dsc").</summary>
        public List<string> SortDirections { get; set; } = ["asc", "dsc"];
        /// <summary>The currently active sort, or <see langword="null"/> if no sort is applied.</summary>
        public CurrentSort? CurrentSort { get; set; }
    }

    /// <summary>
    /// Represents the currently active sort column and direction.
    /// </summary>
    public class CurrentSort
    {
        /// <summary>The column name currently being sorted on.</summary>
        public string? Column { get; set; }
        /// <summary>The sort direction ("asc" or "dsc").</summary>
        public string? Direction { get; set; }
    }

    /// <summary>
    /// Represents the outcome of a single-item operation, carrying a value, message, and HTTP status code.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    public class Result<T>
    {
        /// <summary><see langword="true"/> if the operation completed successfully.</summary>
        public bool IsSuccess { get; init; }
        /// <summary><see langword="true"/> if the operation failed.</summary>
        public bool IsFailure => !IsSuccess;
        /// <summary>The HTTP status code associated with the result.</summary>
        public HttpStatusCode? StatusCode { get; init; }
        /// <summary>A human-readable message describing the outcome.</summary>
        public string? Message { get; init; }
        /// <summary>The result value, or <see langword="default"/> on failure.</summary>
        public T? Value { get; init; }


        protected Result(bool isSuccess, T? value, string? message, HttpStatusCode? statusCode)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Value = value;
            Message = message;
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="value">The result value.</param>
        /// <param name="message">Optional success message.</param>
        /// <param name="statusCode">HTTP status code. Defaults to <see cref="HttpStatusCode.OK"/>.</param>
        public static Result<T> Success(T? value, string? message = null, HttpStatusCode? statusCode = HttpStatusCode.OK)
            => new(true, value, message ?? "Operation successful.", statusCode);

        /// <summary>
        /// Creates a failed result with no value.
        /// </summary>
        /// <param name="message">Optional failure message.</param>
        /// <param name="statusCode">HTTP status code. Defaults to <see cref="HttpStatusCode.BadRequest"/>.</param>
        public static Result<T> Failure(string? message = null, HttpStatusCode? statusCode = HttpStatusCode.BadRequest)
            => new(false, default, message ?? "Operation failed.", statusCode);

        /// <summary>
        /// Creates a failed result that still carries a value (e.g. partial data on error).
        /// </summary>
        /// <param name="value">The partial result value.</param>
        /// <param name="message">Optional failure message.</param>
        /// <param name="statusCode">HTTP status code. Defaults to <see cref="HttpStatusCode.BadRequest"/>.</param>
        public static Result<T> FailureWithValue(T? value, string? message = null, HttpStatusCode? statusCode = HttpStatusCode.BadRequest)
            => new(false, value, message ?? "Operation failed.", statusCode);
    }

    /// <summary>
    /// Represents the outcome of a paginated list operation, extending the result with pagination and sorting metadata.
    /// </summary>
    /// <typeparam name="T">The type of the result value (typically a list).</typeparam>
    public class GetAllResult<T>
    {
        /// <summary><see langword="true"/> if the operation completed successfully.</summary>
        public bool IsSuccess { get; init; }
        /// <summary><see langword="true"/> if the operation failed.</summary>
        public bool IsFailure => !IsSuccess;
        /// <summary>The HTTP status code associated with the result.</summary>
        public HttpStatusCode? StatusCode { get; init; }
        /// <summary>A human-readable message describing the outcome.</summary>
        public string? Message { get; init; }
        /// <summary>The result value, or <see langword="default"/> on failure.</summary>
        public T? Value { get; init; }
        /// <summary>Pagination metadata for the returned list, or <see langword="null"/> if not paginated.</summary>
        public PaginationInfo? Pagination { get; init; }
        /// <summary>Sorting metadata for the returned list, or <see langword="null"/> if sorting info was not provided.</summary>
        public SortInfo? Sorting { get; set; }

        protected GetAllResult(bool isSuccess, T? value, string? message, HttpStatusCode? statusCode, PaginationInfo? paginationInfo = null, SortInfo? sorting = null)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Value = value;
            Message = message;
            Pagination = paginationInfo;
            Sorting = sorting;
        }

        /// <summary>
        /// Creates a successful paginated result.
        /// </summary>
        /// <param name="value">The result list value.</param>
        /// <param name="message">Optional success message.</param>
        /// <param name="statusCode">HTTP status code. Defaults to <see cref="HttpStatusCode.OK"/>.</param>
        /// <param name="paginationInfo">Optional pagination metadata.</param>
        /// <param name="sortInfo">Optional sort metadata.</param>
        public static GetAllResult<T> Success(T? value, string? message = null, HttpStatusCode? statusCode = HttpStatusCode.OK, PaginationInfo? paginationInfo = null, SortInfo? sortInfo = null)
            => new(true, value, message ?? "Operation successful.", statusCode, paginationInfo, sortInfo);

        /// <summary>
        /// Creates a failed paginated result with no value.
        /// </summary>
        /// <param name="message">Optional failure message.</param>
        /// <param name="statusCode">HTTP status code. Defaults to <see cref="HttpStatusCode.BadRequest"/>.</param>
        /// <param name="paginationInfo">Optional pagination metadata.</param>
        /// <param name="sortInfo">Optional sort metadata.</param>
        public static GetAllResult<T> Failure(string? message = null, HttpStatusCode? statusCode = HttpStatusCode.BadRequest, PaginationInfo? paginationInfo = null, SortInfo? sortInfo = null)
            => new(false, default, message ?? "Operation failed.", statusCode, paginationInfo, sortInfo);

        /// <summary>
        /// Creates a failed paginated result that still carries a value.
        /// </summary>
        /// <param name="value">The partial result value.</param>
        /// <param name="message">Optional failure message.</param>
        /// <param name="statusCode">HTTP status code. Defaults to <see cref="HttpStatusCode.BadRequest"/>.</param>
        /// <param name="paginationInfo">Optional pagination metadata.</param>
        /// <param name="sortInfo">Optional sort metadata.</param>
        public static GetAllResult<T> FailureWithValue(T? value, string? message = null, HttpStatusCode? statusCode = HttpStatusCode.BadRequest, PaginationInfo? paginationInfo = null, SortInfo? sortInfo = null)
            => new(false, value, message ?? "Operation failed.", statusCode, paginationInfo, sortInfo);
    }
}