pushd submodules/odata.net
git apply ../../patches/01.RemoveFramework45.patch
git apply ../../patches/02.AddContentInspection.patch
git add -A
git commit -m "TenForce Extensions"
popd