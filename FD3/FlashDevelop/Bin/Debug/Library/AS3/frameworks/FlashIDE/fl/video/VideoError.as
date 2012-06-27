﻿package fl.video
{
	/**
	 * The VideoError exception is the primary mechanism for reporting runtime errors from the 
	 */
	public class VideoError extends Error
	{
		/**
		 * @private
		 */
		static const BASE_ERROR_CODE : uint = 1000;
		/**
		 * State variable indicating that Flash Player is unable to make a connection to the server 
		 */
		public static const NO_CONNECTION : uint = 1000;
		/**
		 * State variable indicating the illegal cue point.
		 */
		public static const ILLEGAL_CUE_POINT : uint = 1002;
		/**
		 * State variable indicating an invalid seek.
		 */
		public static const INVALID_SEEK : uint = 1003;
		/**
		 * State variable indicating an invalid source.
		 */
		public static const INVALID_SOURCE : uint = 1004;
		/**
		 * State variable indicating invalid XML.
		 */
		public static const INVALID_XML : uint = 1005;
		/**
		 * State variable indicating that there is no bitrate match.
		 */
		public static const NO_BITRATE_MATCH : uint = 1006;
		/**
		 * State variable indicating that the user cannot delete the default VideoPlayer object.
		 */
		public static const DELETE_DEFAULT_PLAYER : uint = 1007;
		/**
		 * State variable indicating that the INCManager class is not set.
		 */
		public static const INCMANAGER_CLASS_UNSET : uint = 1008;
		/**
		 * State variable indicating that a <code>null</code> URL was sent to the 
		 */
		public static const NULL_URL_LOAD : uint = 1009;
		/**
		 * State variable indicating a missing skin style.
		 */
		public static const MISSING_SKIN_STYLE : uint = 1010;
		/**
		 * State variable indicating that an unsupported property was passed to the 
		 */
		public static const UNSUPPORTED_PROPERTY : uint = 1011;
		private var _code : uint;
		/**
		 * An error that occurs when the <code>VideoPlayer.netStatusClientClass</code>
		 */
		public static const NETSTREAM_CLIENT_CLASS_UNSET : uint = 1012;
		/**
		 * @private
		 */
		static const ERROR_MSG : Array = [];

		/**
		 * The code that corresponds to the error. The error code is passed into the constructor.
		 */
		public function get code () : uint;

		/**
		 * Creates a new VideoError object.
		 */
		public function VideoError (errCode:uint, msg:String = null);
	}
}