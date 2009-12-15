package flash.html.script
{
	import flash.utils.Proxy;
	import flash.html.script.Package;
	import flash.system.ApplicationDomain;

	public class Package extends Proxy
	{
		public function callProperty (name:*, ...rest) : *;

		public function getProperty (name:*) : *;

		public function hasProperty (name:*) : Boolean;

		public function nextNameIndex (index:int) : int;

		public function Package (parent:Package, packageName:String, appDomain:ApplicationDomain);
	}
}