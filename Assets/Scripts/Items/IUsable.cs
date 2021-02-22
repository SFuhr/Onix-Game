namespace Items
{
    public interface IUsable
    {
        BaseItem GetItem();
        void Use();
        void Destroy();
    }
}