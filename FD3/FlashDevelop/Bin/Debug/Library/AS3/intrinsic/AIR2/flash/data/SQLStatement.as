package flash.data
{
	import flash.events.EventDispatcher;
	import flash.data.SQLConnection;
	import flash.net.Responder;
	import flash.data.SQLResult;

	/**
	 * Dispatched when an error occurs during an operation.
	 * @eventType flash.events.SQLErrorEvent.ERROR
	 */
	[Event(name="error", type="flash.events.SQLErrorEvent")] 

	/**
	 * Dispatched when an execute() or next() method call's operation completes successfully.
	 * @eventType flash.events.SQLEvent.RESULT
	 */
	[Event(name="result", type="flash.events.SQLEvent")] 

	/// A SQLStatement instance is used to execute a SQL statement against a local SQL database that is open through a SQLConnection instance.
	public class SQLStatement extends EventDispatcher
	{
		/// Indicates whether the statement is currently executing.
		public function get executing () : Boolean;

		/// Indicates a class (data type) that is used for each row returned as a result of the statement's execution.
		public function get itemClass () : Class;
		public function set itemClass (value:Class) : void;

		/// Serves as an associative array to which you add values for the parameters specified in the SQL statement's text property.
		public function get parameters () : Object;

		/// The SQLConnection object that manages the connection to the database or databases on which the statement is executed.
		public function get sqlConnection () : SQLConnection;
		public function set sqlConnection (value:SQLConnection) : void;

		/// The actual SQL text of the statement.
		public function get text () : String;
		public function set text (value:String) : void;

		/// Cancels execution of this statement.
		public function cancel () : void;

		/// Clears all current parameter settings.
		public function clearParameters () : void;

		/// Executes the SQL in the text property against the database that is connected to the SQLConnection object in the sqlConnection property.
		public function execute (prefetch:int = -1, responder:Responder = null) : void;

		/// Provides access to a SQLResult object containing the results of the statement execution, including any result rows from a SELECT statement, and other information about the statement execution for all executed statements.
		public function getResult () : SQLResult;

		/// Retrieves the next portion of a SELECT statement's result set.
		public function next (prefetch:int = -1, responder:Responder = null) : void;

		/// Creates a SQLStatement instance.
		public function SQLStatement ();
	}
}