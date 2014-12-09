package sensortest;


public class DeviceData_DeviceDataWrapper
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("SensorTest.DeviceData/DeviceDataWrapper, SensorTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DeviceData_DeviceDataWrapper.class, __md_methods);
	}


	public DeviceData_DeviceDataWrapper () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DeviceData_DeviceDataWrapper.class)
			mono.android.TypeManager.Activate ("SensorTest.DeviceData/DeviceDataWrapper, SensorTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
