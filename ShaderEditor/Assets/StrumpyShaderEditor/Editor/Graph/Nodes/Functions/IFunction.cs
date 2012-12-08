namespace StrumpyShaderEditor
{
	public interface IFunction : IResultCacheNode {
		string GetFunctionDefinition();
		string FunctionName{ get; }
	}
}