pushd submodules/odata.net
git apply ../../patches/01-remove-framework-45.patch
git apply ../../patches/02-add-payload-inspection.patch
git apply ../../patches/03-deep-insert-nested-entities.patch
git add -A
git commit -m "TenForce Extensions"
popd