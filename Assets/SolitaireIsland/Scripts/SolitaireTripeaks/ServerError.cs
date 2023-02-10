namespace SolitaireTripeaks
{
	public enum ServerError
	{
		Unknown = 0,
		None = 1,
		NetworkError = 400,
		ServerNotAvailable = 403,
		NotFound = 404,
		Timeout = 408,
		InternalError = 500,
		NoPermission = 503,
		InvalidParameter = 1001,
		NotSupported = 1002,
		Cheating = 9999,
		ClubNotFound = 10001,
		ClubIsFull = 10002,
		AlreadyJoinedClub = 10003,
		InvitationNotFound = 10004,
		GiftNotFound = 10005,
		NotInClub = 10006
	}
}
