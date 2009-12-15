package flash.events
{
	import flash.events.Event;

	/// A ProgressEvent object is dispatched when a load operation has begun or a socket has received data.
	public class ProgressEvent extends Event
	{
		/// Defines the value of the type property of a progress event object.
		public static const PROGRESS : String = "progress";
		/// Defines the value of the type property of a socketData event object.
		public static const SOCKET_DATA : String = "socketData";
		public static const STANDARD_ERROR_DATA : String = "standardErrorData";
		public static const STANDARD_INPUT_PROGRESS : String = "standardInputProgress";
		public static const STANDARD_OUTPUT_DATA : String = "standardOutputData";

		/// The number of items or bytes loaded when the listener processes the event.
		public function get bytesLoaded () : Number;
		public function set bytesLoaded (value:Number) : void;

		/// The total number of items or bytes that will be loaded if the loading process succeeds.
		public function get bytesTotal () : Number;
		public function set bytesTotal (value:Number) : void;

		/// Creates a copy of the ProgressEvent object and sets each property's value to match that of the original.
		public function clone () : Event;

		/// Constructor for ProgressEvent objects.
		public function ProgressEvent (type:String, bubbles:Boolean = false, cancelable:Boolean = false, bytesLoaded:Number = 0, bytesTotal:Number = 0);

		/// Returns a string that contains all the properties of the ProgressEvent object.
		public function toString () : String;
	}
}