namespace Flian
{
	public interface IEngine
	{
		Position Calculate(Board board, CellValue color);
	}
}