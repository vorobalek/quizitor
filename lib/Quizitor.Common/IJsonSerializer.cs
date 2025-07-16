namespace Quizitor.Common;

public interface IJsonSerializer<T>
{
    string Serialize(T @object);
    T Deserialize(string json);
    T? TryDeserialize(string json);
}