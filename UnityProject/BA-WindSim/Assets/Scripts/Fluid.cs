using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class Fluid : MonoBehaviour 
{
	public ComputeShader shader;
	public Material matResult;
	public Material veloResult;
	public int size = 3840;
	public Transform sphere; //represents mouse
	public int solverIterations = 50;
	public Texture2D obstacleTex;
	public RenderTexture rObstacleTex;
	public bool isTUI = true;

	[Header("Force Settings")]
	public float forceIntensity = 200f;
	public float forceRange = 0.01f;
	private Vector2 sphere_prevPos = Vector3.zero;
	public Color dyeColor = Color.white;
	public Vector2 globalWindDir;

	[Range(0.0f, 0.01f)]
	public float globalWindStrenght = 0.001f;

	public Vector2 veloAtPoint;

	public RenderTexture velocityTex;
	private RenderTexture densityTex;
	private RenderTexture pressureTex;
	private RenderTexture divergenceTex;
	private Texture2D tex;
	public Texture2D veloTex;

	private int dispatchSize = 0;
	private int kernelCount = 0;
	private int kernel_Init = 0;
	private int kernel_Diffusion = 0;
	private int kernel_GlobalVelo = 0;
	private int kernel_Jacobi = 0;
	private int kernel_Advection = 0;
	private int kernel_Divergence = 0;
	private int kernel_SubtractGradient = 0;
	private int kernel_UserInput = 0;

	private RenderTexture CreateTexture(GraphicsFormat format)
	{
		//Texture2D tex = new Texture2D(1024, 1024, TextureFormat.RGB24, false);
		RenderTexture dataTex = new RenderTexture (size, size, 0, format);
		dataTex.filterMode = FilterMode.Bilinear;
		dataTex.wrapMode = TextureWrapMode.Clamp;
		dataTex.enableRandomWrite = true;
		dataTex.Create ();

		return dataTex;
	}

	private Texture2D CreateTexture2D(RenderTexture rTex)
	{
		Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        var old_rt = RenderTexture.active;
        RenderTexture.active = rTex;

        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();

        RenderTexture.active = old_rt;
        return tex;
	}

	private void DispatchCompute(int kernel)
	{
		shader.Dispatch (kernel, dispatchSize, dispatchSize, 1);
	}

	void Start () 
	{
		
		//Create textures
		//velocityTex = CreateTexture(GraphicsFormat.R16G16_SFloat); //float2 velocity
		densityTex = CreateTexture(GraphicsFormat.R16G16B16A16_SFloat); //float3 color , float density
		pressureTex = CreateTexture(GraphicsFormat.R16_SFloat); //float pressure
		divergenceTex = CreateTexture(GraphicsFormat.R16_SFloat); //float divergence

		//Output
		matResult.SetTexture ("_MainTex", densityTex );
		veloResult.SetTexture ("_MainTex", velocityTex );

		//Set shared variables for compute shader
		shader.SetInt("size",size);
		shader.SetFloat("forceIntensity",forceIntensity);
		shader.SetFloat("forceRange",forceRange);

		

		//Set texture for compute shader
		kernel_Init = shader.FindKernel ("Kernel_Init"); kernelCount++;
		kernel_Diffusion = shader.FindKernel ("Kernel_Diffusion"); kernelCount++;
		kernel_UserInput = shader.FindKernel ("Kernel_UserInput"); kernelCount++;
		kernel_Divergence = shader.FindKernel ("Kernel_Divergence"); kernelCount++;
		kernel_Jacobi = shader.FindKernel ("Kernel_Jacobi"); kernelCount++;
		kernel_Advection = shader.FindKernel ("Kernel_Advection"); kernelCount++;
		kernel_SubtractGradient = shader.FindKernel ("Kernel_SubtractGradient"); kernelCount++;
		kernel_GlobalVelo = shader.FindKernel ("Kernel_GlobalVelo"); kernelCount++;
		for(int kernel=0; kernel<kernelCount; kernel++)
		{
			/* 
			This example is not optimized, not all kernels read/write into all textures,
			but I keep it like this for the sake of convenience
			*/
			shader.SetTexture (kernel, "VelocityTex", velocityTex);
			shader.SetTexture (kernel, "DensityTex", densityTex);
			shader.SetTexture (kernel, "PressureTex", pressureTex);
			shader.SetTexture (kernel, "DivergenceTex", divergenceTex);
			shader.SetTexture (kernel, "ObstacleTex", rObstacleTex);
		}

		//Init data texture value
		dispatchSize = Mathf.CeilToInt(size / 16);
		DispatchCompute (kernel_Init);		
	}

	void FixedUpdate()
	{
		
		for(int kernel=0; kernel<kernelCount; kernel++){
			shader.SetTexture (kernel, "ObstacleTex", rObstacleTex);
		}
			
		//Send sphere (mouse) position
		Vector2 npos = new Vector2( sphere.position.x / transform.localScale.x, sphere.position.z / transform.localScale.z );
		shader.SetVector("spherePos",npos);

		//Send sphere (mouse) velocity
		//Vector2 velocity = npos - sphere_prevPos;
		Vector2 velocity = new Vector2(0f, 0f);
		shader.SetVector("sphereVelocity",velocity);
		shader.SetFloat("_deltaTime", Time.fixedDeltaTime);
		shader.SetVector("dyeColor",dyeColor);

		//Run compute shader
		DispatchCompute (kernel_GlobalVelo);
		DispatchCompute (kernel_Diffusion);
		DispatchCompute (kernel_Advection);
		if(Input.GetMouseButton(0) && (Input.GetKey(KeyCode.LeftControl) || isTUI)){DispatchCompute (kernel_UserInput);}
		
		DispatchCompute (kernel_Divergence);
		for(int i=0; i<solverIterations; i++)
		{
			DispatchCompute (kernel_Jacobi);
		}
		DispatchCompute (kernel_SubtractGradient);
		
		//Save the previous position for velocity
		sphere_prevPos = npos;

		
		Vector2 globalVelo = globalWindDir * globalWindStrenght;
		//Debug.Log(globalWindDir * globalWindStrenght);
		shader.SetVector("_globalVelo", globalVelo);

		//if (Input.GetKeyDown("space"))
        //{
			//npos ggf gegen WindPark Pos austauschen
			//getWindAtPoint((int) map(npos.x, -0.28f, 0.28f, 840f, 3000f), (int) map(npos.y, -0.50f, 0.50f, 0f, 3840f));
        //}
	}

	public void setglobalWindDir(float x, float y)
	{
		globalWindDir = new Vector2(x, y).normalized;
		//Debug.Log("New WindDir: " + globalWindDir);
	}

	public Vector2 mapGUIPostion(float x, float y){
		return new Vector2 (map(x / transform.localScale.x, -0.28f, 0.28f, 840f, 3000f), map(y / transform.localScale.z, -0.50f, 0.50f, 0f, 3840f));
	}

	public Vector2 mapTUIPostion(float x, float y){
        return new Vector2 (map(x, 102f, 3728f, 0f, 3840f), map(y, 76.5f, 2079f, 840f, 3000f));
    }

	Color CalculateAverageColor(Color c1, Color c2, Color c3, Color c4)
    {
        float r = (c1.r + c2.r + c3.r + c4.r) / 4f;
        float g = (c1.g + c2.g + c3.g + c4.g) / 4f;
        float b = (c1.b + c2.b + c3.b + c4.b) / 4f;
        float a = (c1.a + c2.a + c3.a + c4.a) / 4f;

        return new Color(r, g, b, a);
    }

	float map(float s, float a1, float a2, float b1, float b2)
	{
    	return b1 + (s-a1)*(b2-b1)/(a2-a1);
	}

	public RenderTexture GetVeloRTex(){
		return velocityTex;
	}
}