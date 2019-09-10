public class OctantTransform
{
    public int xx { get; private set; }
    public int xz { get; private set; }
    public int zx { get; private set; }
    public int zz { get; private set; }

    public OctantTransform(int xx, int xz, int zx, int zz)
    {
        this.xx = xx;
        this.xz = xz;
        this.zx = zx;
        this.zz = zz;
    }

    public override string ToString()
    {
        return string.Format("[OctantTransform {0,2:D} {1,2:D} {2,2:D} {3,2:D}]",
            xx, xz, zx, zz);
    }
}