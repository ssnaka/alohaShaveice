#pragma warning disable 612,618
#pragma warning disable 0114
#pragma warning disable 0108

using System;
using System.Collections.Generic;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

//THIS FILE IS AUTO GENERATED, DO NOT MODIFY!!
//THIS FILE IS AUTO GENERATED, DO NOT MODIFY!!
//THIS FILE IS AUTO GENERATED, DO NOT MODIFY!!

namespace GameSparks.Api.Requests{
		public class LogEventRequest_AddCurrency : GSTypedRequest<LogEventRequest_AddCurrency, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_AddCurrency() : base("LogEventRequest"){
			request.AddString("eventKey", "AddCurrency");
		}
		public LogEventRequest_AddCurrency Set_Value( long value )
		{
			request.AddNumber("Value", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_AddCurrency : GSTypedRequest<LogChallengeEventRequest_AddCurrency, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_AddCurrency() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "AddCurrency");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_AddCurrency SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_AddCurrency Set_Value( long value )
		{
			request.AddNumber("Value", value);
			return this;
		}			
	}
	
	public class LogEventRequest_ScoreLevel : GSTypedRequest<LogEventRequest_ScoreLevel, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_ScoreLevel() : base("LogEventRequest"){
			request.AddString("eventKey", "ScoreLevel");
		}
		public LogEventRequest_ScoreLevel Set_Level( long value )
		{
			request.AddNumber("Level", value);
			return this;
		}			
		public LogEventRequest_ScoreLevel Set_Score( long value )
		{
			request.AddNumber("Score", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_ScoreLevel : GSTypedRequest<LogChallengeEventRequest_ScoreLevel, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_ScoreLevel() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "ScoreLevel");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_ScoreLevel SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_ScoreLevel Set_Level( long value )
		{
			request.AddNumber("Level", value);
			return this;
		}			
		public LogChallengeEventRequest_ScoreLevel Set_Score( long value )
		{
			request.AddNumber("Score", value);
			return this;
		}			
	}
	
}
	
	
	
namespace GameSparks.Api.Requests{
	
	public class LeaderboardDataRequest_Level : GSTypedRequest<LeaderboardDataRequest_Level,LeaderboardDataResponse_Level>
	{
		public LeaderboardDataRequest_Level() : base("LeaderboardDataRequest"){
			request.AddString("leaderboardShortCode", "Level");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LeaderboardDataResponse_Level (response);
		}		
		
		/// <summary>
		/// The challenge instance to get the leaderboard data for
		/// </summary>
		public LeaderboardDataRequest_Level SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		/// <summary>
		/// The number of items to return in a page (default=50)
		/// </summary>
		public LeaderboardDataRequest_Level SetEntryCount( long entryCount )
		{
			request.AddNumber("entryCount", entryCount);
			return this;
		}
		/// <summary>
		/// A friend id or an array of friend ids to use instead of the player's social friends
		/// </summary>
		public LeaderboardDataRequest_Level SetFriendIds( List<string> friendIds )
		{
			request.AddStringList("friendIds", friendIds);
			return this;
		}
		/// <summary>
		/// Number of entries to include from head of the list
		/// </summary>
		public LeaderboardDataRequest_Level SetIncludeFirst( long includeFirst )
		{
			request.AddNumber("includeFirst", includeFirst);
			return this;
		}
		/// <summary>
		/// Number of entries to include from tail of the list
		/// </summary>
		public LeaderboardDataRequest_Level SetIncludeLast( long includeLast )
		{
			request.AddNumber("includeLast", includeLast);
			return this;
		}
		
		/// <summary>
		/// The offset into the set of leaderboards returned
		/// </summary>
		public LeaderboardDataRequest_Level SetOffset( long offset )
		{
			request.AddNumber("offset", offset);
			return this;
		}
		/// <summary>
		/// If True returns a leaderboard of the player's social friends
		/// </summary>
		public LeaderboardDataRequest_Level SetSocial( bool social )
		{
			request.AddBoolean("social", social);
			return this;
		}
		/// <summary>
		/// The IDs of the teams you are interested in
		/// </summary>
		public LeaderboardDataRequest_Level SetTeamIds( List<string> teamIds )
		{
			request.AddStringList("teamIds", teamIds);
			return this;
		}
		/// <summary>
		/// The type of team you are interested in
		/// </summary>
		public LeaderboardDataRequest_Level SetTeamTypes( List<string> teamTypes )
		{
			request.AddStringList("teamTypes", teamTypes);
			return this;
		}
		
	}

	public class AroundMeLeaderboardRequest_Level : GSTypedRequest<AroundMeLeaderboardRequest_Level,AroundMeLeaderboardResponse_Level>
	{
		public AroundMeLeaderboardRequest_Level() : base("AroundMeLeaderboardRequest"){
			request.AddString("leaderboardShortCode", "Level");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new AroundMeLeaderboardResponse_Level (response);
		}		
		
		/// <summary>
		/// The number of items to return in a page (default=50)
		/// </summary>
		public AroundMeLeaderboardRequest_Level SetEntryCount( long entryCount )
		{
			request.AddNumber("entryCount", entryCount);
			return this;
		}
		/// <summary>
		/// A friend id or an array of friend ids to use instead of the player's social friends
		/// </summary>
		public AroundMeLeaderboardRequest_Level SetFriendIds( List<string> friendIds )
		{
			request.AddStringList("friendIds", friendIds);
			return this;
		}
		/// <summary>
		/// Number of entries to include from head of the list
		/// </summary>
		public AroundMeLeaderboardRequest_Level SetIncludeFirst( long includeFirst )
		{
			request.AddNumber("includeFirst", includeFirst);
			return this;
		}
		/// <summary>
		/// Number of entries to include from tail of the list
		/// </summary>
		public AroundMeLeaderboardRequest_Level SetIncludeLast( long includeLast )
		{
			request.AddNumber("includeLast", includeLast);
			return this;
		}
		
		/// <summary>
		/// If True returns a leaderboard of the player's social friends
		/// </summary>
		public AroundMeLeaderboardRequest_Level SetSocial( bool social )
		{
			request.AddBoolean("social", social);
			return this;
		}
		/// <summary>
		/// The IDs of the teams you are interested in
		/// </summary>
		public AroundMeLeaderboardRequest_Level SetTeamIds( List<string> teamIds )
		{
			request.AddStringList("teamIds", teamIds);
			return this;
		}
		/// <summary>
		/// The type of team you are interested in
		/// </summary>
		public AroundMeLeaderboardRequest_Level SetTeamTypes( List<string> teamTypes )
		{
			request.AddStringList("teamTypes", teamTypes);
			return this;
		}
	}
}

namespace GameSparks.Api.Responses{
	
	public class _LeaderboardEntry_Level : LeaderboardDataResponse._LeaderboardData{
		public _LeaderboardEntry_Level(GSData data) : base(data){}
	}
	
	public class LeaderboardDataResponse_Level : LeaderboardDataResponse
	{
		public LeaderboardDataResponse_Level(GSData data) : base(data){}
		
		public GSEnumerable<_LeaderboardEntry_Level> Data_Level{
			get{return new GSEnumerable<_LeaderboardEntry_Level>(response.GetObjectList("data"), (data) => { return new _LeaderboardEntry_Level(data);});}
		}
		
		public GSEnumerable<_LeaderboardEntry_Level> First_Level{
			get{return new GSEnumerable<_LeaderboardEntry_Level>(response.GetObjectList("first"), (data) => { return new _LeaderboardEntry_Level(data);});}
		}
		
		public GSEnumerable<_LeaderboardEntry_Level> Last_Level{
			get{return new GSEnumerable<_LeaderboardEntry_Level>(response.GetObjectList("last"), (data) => { return new _LeaderboardEntry_Level(data);});}
		}
	}
	
	public class AroundMeLeaderboardResponse_Level : AroundMeLeaderboardResponse
	{
		public AroundMeLeaderboardResponse_Level(GSData data) : base(data){}
		
		public GSEnumerable<_LeaderboardEntry_Level> Data_Level{
			get{return new GSEnumerable<_LeaderboardEntry_Level>(response.GetObjectList("data"), (data) => { return new _LeaderboardEntry_Level(data);});}
		}
		
		public GSEnumerable<_LeaderboardEntry_Level> First_Level{
			get{return new GSEnumerable<_LeaderboardEntry_Level>(response.GetObjectList("first"), (data) => { return new _LeaderboardEntry_Level(data);});}
		}
		
		public GSEnumerable<_LeaderboardEntry_Level> Last_Level{
			get{return new GSEnumerable<_LeaderboardEntry_Level>(response.GetObjectList("last"), (data) => { return new _LeaderboardEntry_Level(data);});}
		}
	}
}	

namespace GameSparks.Api.Messages {


}
