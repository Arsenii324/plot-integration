namespace winforproj;
public partial class Form1 : Form
{
    // ========  ==
    bool forcerectangle = false; //if true, OX and OY have same proportions and force fits min; else fits the whole range on screen
    double eps1 = 1e10, eps2 = 1;
    float penwidth = 0.02f; //width of pen
    const int N = 1 << 7; //amount of points on the graph; use a power of two
    bool may_continue;
    double x0;
    double a, b, y;
    public double F(double x, double y)
    {
        Func<double, double, double> Calc =
            //Add(C(-1), Mul(Mul(X, X), IntegralY(C(0), C(Math.PI / 2), Mul(Pow(C(Math.E), Mul(C(-0.5), Mul(Apply(Math.Cos, Y), Mul(X, X)))), Pow(Apply(Math.Sin, Y), C(0.5))))));
            Add(Mul(C(2), X), Mul(C(-1), IntegralY(C(-0.99), X, Div(C(1), Mul(Add(Y, C(2)), Pow(Apply(Math.Log, Add(Y, C(2))), C(0.5)))))));
        return Calc(x, y);
    }
    // ========  ==
    public Func<double, double, double> C(double c)
    {
        double ConstVal(double x, double y)
        {
            return c;
        }
        return ConstVal;
    }
    public double X(double x, double y)
    {
        return x;
    }
    public double Y(double x, double y)
    {
        return y;
    }
    public Func<double, double, double> Add(Func<double, double, double> f1, Func<double, double, double> f2)
    {
        double Add_1(double x, double y)
        {
            return f1(x, y) + f2(x, y);
        }
        return Add_1;
    }
    public Func<double, double, double> Sub(Func<double, double, double> f1, Func<double, double, double> f2)
    {
        double Sub_1(double x, double y)
        {
            return f1(x, y) - f2(x, y);
        }
        return Sub_1;
    }
    public Func<double, double, double> Mul(Func<double, double, double> f1, Func<double, double, double> f2)
    {
        double Mul_1(double x, double y)
        {
            return f1(x, y) * f2(x, y);
        }
        return Mul_1;
    }
    public Func<double, double, double> Div(Func<double, double, double> f1, Func<double, double, double> f2)
    {
        double Div_1(double x, double y)
        {
            return f1(x, y) / f2(x, y);
        }
        return Div_1;
    }
    public Func<double, double, double> Log(Func<double, double, double> f1, Func<double, double, double> f2)
    {
        double Log_1(double x, double y)
        {
            return Math.Log(f2(x, y)) / Math.Log(f1(x, y));
        }
        return Log_1;
    }
    public Func<double, double, double> Pow(Func<double, double, double> f1, Func<double, double, double> f2)
    {
        double Pow_1(double x, double y)
        {
            return Math.Pow(f1(x, y), f2(x, y));
        }
        return Pow_1;
    }
    public Func<double, double, double> Apply(Func<double, double> apply, Func<double, double, double> f1)
    {
        double Apply_1(double x, double y)
        {
            return apply(f1(x, y));
        }
        return Apply_1;
    }
    public Func<double, double, double> Apply(Func<double, double, double> apply,
        Func<double, double, double> f1, Func<double, double, double> f2)
    {
        double Apply_1(double x, double y)
        {
            return apply(f1(x, y), f2(x, y));
        }
        return Apply_1;
    }

    public Func<double, double, double> IntegralX(Func<double, double, double> fbeg, Func<double, double, double> fend,
        Func<double, double, double> f1)
    {
        double FindEps(double x1, double x2, double y)
        {
            return Math.Abs((x2 - x1) * (f1(x2, y) - f1(x1, y)));
        }
        double Integ_Slow(double x, double y)
        {
            double cx = fbeg(x, y);
            double lx = fend(x, y);
            double ans = 0;
            bool rv = (cx > lx);
            if (rv)
            {
                double t = lx;
                lx = cx;
                cx = t;
            }
            double nx = 1;
            while (FindEps(cx, cx + nx, y) > eps1) nx /= 2;
            while (FindEps(cx, cx + nx * 2, y) < eps1 && cx + nx < lx) nx *= 2;

            while (cx + nx < lx)
            {
                ans += nx * (f1(cx, y) + f1(cx + nx, y)) / 2;
                cx += nx;
                while (FindEps(cx, cx + nx, y) > eps1) nx /= 2;
                while (FindEps(cx, cx + nx * 2, y) < eps1 && cx + nx < lx) nx *= 2;
            }
            ans += (lx - cx) * (f1(cx, y) + f1(lx, y)) / 2;
            if (rv) ans = -ans;
            return ans;
        }
        return Integ_Slow;
    }
    public Func<double, double, double> IntegralY(Func<double, double, double> fbeg, Func<double, double, double> fend,
        Func<double, double, double> f1)
    {
        double FindEps(double x1, double x2, double x)
        {
            return Math.Abs((x2 - x1) * (f1(x, x1) - f1(x, x2)));
        }
        double Integ_Slow(double x, double y)
        {
            double cx = fbeg(x, y);
            double lx = fend(x, y);
            double ans = 0;
            bool rv = (cx > lx);
            if (rv)
            {
                double t = lx;
                lx = cx;
                cx = t;
            }
            double nx = 1;
            while (FindEps(cx, cx + nx, x) > eps1) nx /= 2;
            while (FindEps(cx, cx + nx * 2, x) < eps1) nx *= 2;

            while (cx + nx < lx)
            {
                ans += nx * (f1(x, cx) + f1(x, cx + nx)) / 2;
                cx += nx;
                while (FindEps(cx, cx + nx, x) > eps1) nx /= 2;
                while (FindEps(cx, cx + nx * 2, x) < eps1) nx *= 2;
            }
            ans += (lx - cx) * (f1(x, cx) + f1(x, lx)) / 2;
            if (rv) ans = -ans;
            return ans;
        }
        return Integ_Slow;
    }


    static private int ToInt(double x)
    {
        return (int)Math.Round(x);
    }


    double width, height;
    int margin = 3;
    public Form1()
    {
        InitializeComponent();
        width = pictureBox2.Size.Width - margin * 2;
        height = pictureBox2.Size.Height - margin * 2;
        //xd = width / 2;
        //vd = pictureBox2.Size.Height / 2;
        //llim = (xc - xd) * pmulx;
        //rlim = (xc + xd) * pmulx;
    }
    Graphics? graphics = null;
    Pen? pen = null;
    double pmulx, pmuly, miny, maxy;
    double y2;
    Point Bpoint;
    double getx(double x)
    {
        return (x - a) * pmulx + margin;
    }
    double gety(double y)
    {
        return height - (y - miny) * pmuly + margin;
    }
    int itercnt = 0;
    private void button1_Click(object sender, EventArgs e)
    {
        itercnt = 0;
        button2.Text = "Итерация";
        textBox7.Text = "1";
        label9.Text = Convert.ToString(itercnt).PadLeft(6);
        may_continue = true;
        a = Convert.ToDouble(textBox1.Text.Replace('.',','));
        b = Convert.ToDouble(textBox2.Text.Replace('.', ','));
        eps1 = Convert.ToDouble(textBox3.Text.Replace('.', ','));
        eps2 = Convert.ToDouble(textBox4.Text.Replace('.', ','));
        y = Convert.ToDouble(textBox5.Text.Replace('.', ','));
        x0 = Convert.ToDouble(textBox6.Text.Replace('.', ','));
        label11.Text = Convert.ToString(x0);
        //xc = (a + b) / 2;
        pmulx = width / (b - a);
        miny = F(a, y);
        maxy = F(b, y);
        graphics = pictureBox2.CreateGraphics();
        graphics.Clear(Color.White);
        pen = new Pen(Color.Black,penwidth);

        Point[] points = new Point[N+1];
        int p = 0;
        for (double cf = 0; cf <= 1; cf += 1.0/N)
        {
            double nv = F(a * cf + b * (1 - cf), y);
            if (nv > maxy) maxy = nv;
            if (nv < miny) miny = nv;
        }
        label14.Text = Convert.ToString(miny);
        label15.Text = Convert.ToString(maxy);
        
        pmuly = height / (maxy - miny);

        if (forcerectangle) pmuly = pmulx;

        for (double cf = 0; cf <= 1; cf += 1.0 / N)
        {
            double nv = F(a * cf + b * (1 - cf), y);
            if (nv > maxy) maxy = nv;
            if (nv < miny) miny = nv;
            points[p++] = new Point(ToInt(getx(a * cf + b * (1 - cf))), ToInt(gety(nv)));
        }

        y2 = F(b, y);
        graphics.DrawLines(pen, points);
        graphics.DrawLine(pen, new Point(ToInt(getx(a)), ToInt(gety(0))), new Point(ToInt(getx(b)), ToInt(gety(0))));
        Bpoint = new Point(ToInt(getx(b)), ToInt(gety(y2)));
        double y1 = F(x0, y);
        graphics.DrawLine(pen, new Point(ToInt(getx(x0)), ToInt(gety(y1))), Bpoint);
        label17.Text = Convert.ToString(y1);
        if (Math.Min(a, b) <= 0 && Math.Max(a, b) >= 0)
        {
            graphics.DrawLine(pen, new Point(ToInt(getx(0)), ToInt(gety(miny))), new Point(ToInt(getx(0)), ToInt(gety(maxy))));

        }
    }

    private void iterate()
    {

        if (graphics == null || pen == null) return;
        if (!may_continue) return;

        double y1 = F(x0, y);
        if (y1 * y2 >= 0)
        {
            may_continue = false;
            if (y1 == 0) button2.Text = Convert.ToString(x0);
            else if (y2 == 0) button2.Text = Convert.ToString(b);
            else button2.Text = "Концы хорды имеют одинаковый знак: " + (y1 > 0 ? "+" : "-");
            return;
        }
        double xmed = x0 - (1 - eps1) * y1 / ((y2 - y1) / (b - x0));
        label11.Text = Convert.ToString(xmed);
        double ymed = F(xmed, y);
        label17.Text = Convert.ToString(ymed);
        Point MedPoint = new Point(ToInt(getx(xmed)), ToInt(gety(ymed)));
        graphics.DrawLine(pen, MedPoint, new Point(ToInt(getx(xmed)), ToInt(gety(0))));
        graphics.DrawLine(pen, MedPoint, Bpoint);
        if (Math.Sqrt((xmed - x0) * (xmed - x0) + y1 * y1) < eps2)
        {
            may_continue = false;
            button2.Text = Convert.ToString(xmed);
            return;

        }
        x0 = xmed;

        label9.Text = Convert.ToString(++itercnt).PadLeft(6);
    }
    private void button2_Click(object sender, EventArgs e)
    {
        int cnt = Convert.ToInt32(textBox7.Text);
        while (cnt-- > 0) iterate();
    }
}
